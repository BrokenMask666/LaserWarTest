using LaserwarTest.Presentation.Games;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Xfinium.Pdf;
using Xfinium.Pdf.Graphics;

namespace LaserwarTest.Data.PDF
{
    /// <summary>
    /// Используется для генерации PDF-файла с информацией по игре
    /// </summary>
    public sealed class PDFGameInfoGenerator
    {
        const double FONTSIZE_PAGE_TITLE = 20;
        const double FONTSIZE_GROUP_HEADER = 18;
        const double FONTSIZE_DEFAULT = 12;

        const double OFFSET_Y_FROM_TITLE = 60;
        const double OFFSET_Y_FROM_TABLE_HEADER = 40;
        const double OFFSET_Y_FROM_GROUP_HEADER_OR_ITEM = 30;
        const double OFFSET_Y_FROM_GROUP = 40;
        const double OFFSET_Y_FROM_TABLE = 70;

        PdfPage CurrentPage { set; get; }

        /// <summary>
        /// Отступы от краев страницы
        /// </summary>
        Thickness PagePadding { get; } = new Thickness(100, 150, 50, 80);
        Size AvailableSize { set; get; }


        public async Task<PdfFixedDocument> GenerateGamePDF(Game game, IEnumerable<Team> teams, IEnumerable<Player> players)
        {
            CultureInfo currentCulture = CultureInfo.CurrentUICulture;

            PdfUnicodeTrueTypeFont verdanaBold = await CreateFontFromFile("verdanab.ttf", FONTSIZE_PAGE_TITLE);
            PdfUnicodeTrueTypeFont verdana = await CreateFontFromFile("verdana.ttf", FONTSIZE_DEFAULT);

            PdfBrush brush = new PdfBrush();

            PdfFixedDocument pdf = new PdfFixedDocument();
            CreateNewPage(pdf, verdanaBold, brush);

            /// Заголовок документа
            verdanaBold.Size = FONTSIZE_PAGE_TITLE;
            CurrentPage.Graphics.DrawString($"{game.Name.ToUpper()} {game.Date}", verdanaBold, brush, PagePadding.Left, PagePadding.Top);

            MeasureAvailableSpace(offsetY: FONTSIZE_PAGE_TITLE + OFFSET_Y_FROM_TITLE);

            /// Заголовки таблицы
            verdanaBold.Size = FONTSIZE_DEFAULT;
            double columnSize = AvailableSize.Width * 0.25;
            double tableHeaderYPos = GetAvailableTop();
            CurrentPage.Graphics.DrawString("Игрок",    verdanaBold, brush, PagePadding.Left + columnSize * 0, tableHeaderYPos);
            CurrentPage.Graphics.DrawString("Рейтинг",  verdanaBold, brush, PagePadding.Left + columnSize * 1, tableHeaderYPos);
            CurrentPage.Graphics.DrawString("Точность", verdanaBold, brush, PagePadding.Left + columnSize * 2, tableHeaderYPos);
            CurrentPage.Graphics.DrawString("Выстрелы", verdanaBold, brush, PagePadding.Left + columnSize * 3, tableHeaderYPos);

            MeasureAvailableSpace(offsetY: FONTSIZE_DEFAULT + OFFSET_Y_FROM_TABLE_HEADER);

            /// Заполнение данных групп
            PdfPen linePen = new PdfPen(new PdfRgbColor(213, 214, 216), 0.5);
            Dictionary<Team, IEnumerable<Player>> playersTeams = new Dictionary<Team, IEnumerable<Player>>();
            int counter = 0;
            double teamHeaderDesiredHeight = FONTSIZE_GROUP_HEADER + OFFSET_Y_FROM_GROUP_HEADER_OR_ITEM;
            foreach (var team in teams)
            {
                if (counter != 0) MeasureAvailableSpace(offsetY: OFFSET_Y_FROM_GROUP);

                if (AvailableSize.Height < teamHeaderDesiredHeight)
                    CreateNewPage(pdf, verdanaBold, brush);

                verdanaBold.Size = FONTSIZE_GROUP_HEADER;
                CurrentPage.Graphics.DrawString(team.Name, verdanaBold, brush, PagePadding.Left, GetAvailableTop());

                MeasureAvailableSpace(offsetY: teamHeaderDesiredHeight);

                var playersInTeam = playersTeams[team] = players.Where(x => x.TeamID == team.ID);

                double playerDesiredSize = FONTSIZE_DEFAULT + 5 + OFFSET_Y_FROM_GROUP_HEADER_OR_ITEM;
                foreach (var player in playersInTeam)
                {
                    if (AvailableSize.Height < teamHeaderDesiredHeight)
                        CreateNewPage(pdf, verdanaBold, brush);

                    double rowYPos = GetAvailableTop();
                    double lineYpos = rowYPos + FONTSIZE_DEFAULT + 5;

                    CurrentPage.Graphics.DrawString(player.Name,                     verdana, brush, PagePadding.Left + columnSize * 0, rowYPos);
                    CurrentPage.Graphics.DrawString($"{player.Rating}",              verdana, brush, PagePadding.Left + columnSize * 1, rowYPos);
                    CurrentPage.Graphics.DrawString($"{player.AccuracyPercentage}%", verdana, brush, PagePadding.Left + columnSize * 2, rowYPos);
                    CurrentPage.Graphics.DrawString($"{player.Shots}",               verdana, brush, PagePadding.Left + columnSize * 3, rowYPos);

                    CurrentPage.Graphics.DrawLine(linePen, PagePadding.Left, lineYpos, CurrentPage.Width - PagePadding.Right, lineYpos);
                    MeasureAvailableSpace(offsetY: playerDesiredSize);
                }

                counter++;
            }

            /// Блок сравнения команд по показателям
            PdfPen rectanglePen = new PdfPen(new PdfRgbColor(1, 90, 255), 0.5);
            PdfBrush rectangleBrush = new PdfBrush(new PdfRgbColor(1, 90, 255));

            double teamMarkWidth = (AvailableSize.Width - OFFSET_Y_FROM_TABLE) * 0.5;
            double teamMarkDesiredHeight =
                FONTSIZE_GROUP_HEADER +
                OFFSET_Y_FROM_GROUP_HEADER_OR_ITEM +
                FONTSIZE_DEFAULT +
                5 +
                20 +
                OFFSET_Y_FROM_GROUP_HEADER_OR_ITEM +
                5 +
                20;

            counter = 0;
            MeasureAvailableSpace(offsetY: OFFSET_Y_FROM_TABLE);

                double maxRating = players.Max(x => x.Rating);
            foreach (var playersTeam in playersTeams)
            {
                int markXPosIndex = counter % 2;
                if (counter > 0 && markXPosIndex == 0) MeasureAvailableSpace(offsetY: teamMarkDesiredHeight + OFFSET_Y_FROM_TABLE);

                if (markXPosIndex == 0 && AvailableSize.Height < teamMarkDesiredHeight)
                    CreateNewPage(pdf, verdanaBold, brush);

                double xPos = PagePadding.Left + (teamMarkWidth + OFFSET_Y_FROM_TABLE) * markXPosIndex;
                double yPos = GetAvailableTop();

                Team team = playersTeam.Key;
                IEnumerable<Player> playersInTeam = playersTeam.Value;

                verdanaBold.Size = FONTSIZE_GROUP_HEADER;
                CurrentPage.Graphics.DrawString(team.Name, verdanaBold, brush, xPos, yPos);

                /// Рейтинг
                yPos += FONTSIZE_GROUP_HEADER + OFFSET_Y_FROM_GROUP_HEADER_OR_ITEM;
                double avgRating = playersInTeam.Average(x => x.Rating);

                PdfStringAppearanceOptions ratingValueAppearance = new PdfStringAppearanceOptions
                {
                    Brush = brush,
                    Font = verdanaBold,
                };

                PdfStringLayoutOptions ratingValueLayout = new PdfStringLayoutOptions()
                {
                    HorizontalAlign = PdfStringHorizontalAlign.Right,
                    VerticalAlign = PdfStringVerticalAlign.Bottom,
                    X = xPos + teamMarkWidth,
                    Y = yPos
                };

                verdanaBold.Size = FONTSIZE_DEFAULT;
                CurrentPage.Graphics.DrawString("Рейтинг", verdanaBold, brush, xPos, yPos);
                CurrentPage.Graphics.DrawString(avgRating.ToString("0.00", currentCulture), ratingValueAppearance, ratingValueLayout);

                yPos += FONTSIZE_DEFAULT + 5;
                CurrentPage.Graphics.DrawRectangle(rectanglePen, xPos, yPos, teamMarkWidth, 10);
                CurrentPage.Graphics.DrawRectangle(rectanglePen, rectangleBrush, xPos, yPos, avgRating * teamMarkWidth / maxRating, 10);

                /// Точность
                yPos += 10 + OFFSET_Y_FROM_GROUP_HEADER_OR_ITEM;
                double avgAccuracy = playersInTeam.Average(x => x.AccuracyPercentage);

                PdfStringAppearanceOptions accuracyValueAppearance = new PdfStringAppearanceOptions
                {
                    Brush = brush,
                    Font = verdanaBold,
                };

                PdfStringLayoutOptions accuracyValueLayout = new PdfStringLayoutOptions()
                {
                    HorizontalAlign = PdfStringHorizontalAlign.Right,
                    VerticalAlign = PdfStringVerticalAlign.Bottom,
                    X = xPos + teamMarkWidth,
                    Y = yPos
                };

                verdanaBold.Size = FONTSIZE_DEFAULT;
                CurrentPage.Graphics.DrawString("Точность", verdanaBold, brush, xPos, yPos);
                CurrentPage.Graphics.DrawString(avgAccuracy.ToString("0.0", currentCulture) + "%", accuracyValueAppearance, accuracyValueLayout);

                yPos += FONTSIZE_DEFAULT + 5;
                CurrentPage.Graphics.DrawRectangle(rectanglePen, xPos, yPos, teamMarkWidth, 10);
                CurrentPage.Graphics.DrawRectangle(rectanglePen, rectangleBrush, xPos, yPos, avgAccuracy * teamMarkWidth / 100, 10);

                counter++;
            }




            foreach (var page in pdf.Pages)
                page.Graphics.CompressAndClose();

            return pdf;
        }


        void CreateNewPage(PdfFixedDocument document, PdfFont font, PdfBrush brush)
        {
            var page = document.Pages.Add();
            page.Height = 1024;
            page.Width = 840;

            font.Size = FONTSIZE_PAGE_TITLE;

            PdfStringAppearanceOptions appearance = new PdfStringAppearanceOptions
            {
                Brush = brush,
                Font = font,
            };

            PdfStringLayoutOptions layout = new PdfStringLayoutOptions()
            {
                HorizontalAlign = PdfStringHorizontalAlign.Right,
                VerticalAlign = PdfStringVerticalAlign.Bottom,
                X = page.Width - PagePadding.Right,
                Y = page.Height - PagePadding.Bottom
            };

            font.Size = FONTSIZE_DEFAULT;
            page.Graphics.DrawString(document.Pages.Count.ToString(), appearance, layout);

            CurrentPage = page;
            AvailableSize = new Size(
                page.Width - PagePadding.Left - PagePadding.Right,
                page.Height - PagePadding.Top - PagePadding.Bottom);
        }

        async Task<PdfUnicodeTrueTypeFont> CreateFontFromFile(string fontName, double fontSize)
        {
            var ttf = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/Fonts/{fontName}"));
            var ttfStream = await ttf.OpenStreamForReadAsync();

            var font = new PdfUnicodeTrueTypeFont(ttfStream, fontSize, true);
            ttfStream.Dispose();

            return font;
        }

        #region Measurement

        private double GetAvailableTop() => CurrentPage.Height - PagePadding.Bottom - AvailableSize.Height;
        private double GetAvailableLeft() => CurrentPage.Width - PagePadding.Right - AvailableSize.Width;

        public void MeasureAvailableSpace(double offsetX = 0, double offsetY = 0)
        {
            double w = AvailableSize.Width - offsetX;
            double h = AvailableSize.Height - offsetY;

            if (w < 0 || h < 0)
            {
                AvailableSize = new Size();
                return;
            }

            AvailableSize = new Size(w, h);
        }

        #endregion Measurement
    }
}
