using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace LaserwarTest.Commons.UI.Renderer
{
    public class ScreenshotMaker
    {
        /// <summary>
        /// Создает снимок указанного элемента и возвращает в виде изображения
        /// </summary>
        /// <param name="element">Отрисовываемый элемент</param>
        /// <returns></returns>
        public async Task<WriteableBitmap> RenderElement(UIElement element)
        {
            RenderTargetBitmap render = new RenderTargetBitmap();
            await render.RenderAsync(element);

            WriteableBitmap wbm = new WriteableBitmap(render.PixelWidth, render.PixelHeight);

            IBuffer savedBuffer = await render.GetPixelsAsync();
            using (Stream readStream = savedBuffer.AsStream())
            using (Stream writeStream = wbm.PixelBuffer.AsStream())
            {
                await readStream.CopyToAsync(writeStream);
            }

            return wbm;
        }

        /// <summary>
        /// Создает снимок указанного элемента, сохраняет в файл в LocalFolder под указанным именем (включая подпапки и расширение).
        /// Если файл уже существует, заменяет его.
        /// Дополнительно можно указать, чтобы файл сохранялся в библиотеку изображений вместо основного расположения.
        /// Возвращает текст, содержащий полный путь к снимку или сообщение об ошибке
        /// </summary>
        /// <param name="element">Элемент, чье нарисованное состояние записывается в файл</param>
        /// <param name="fileName">Имя файла включая его расширение относительно корневого каталога. Может включать любое количество подпапок корневого каталога</param>
        /// <returns></returns>
        public async Task<string> RenderElementAndSave(UIElement element, string fileName)
        {
            string ret = "";
            try
            {
                RenderTargetBitmap shot;

                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                {
                    shot = new RenderTargetBitmap();
                    await shot.RenderAsync(element);

                    IBuffer pixels = await shot.GetPixelsAsync();
                    float logicalDpi = DisplayInformation.GetForCurrentView().LogicalDpi;
                    BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                    encoder.SetPixelData(
                        BitmapPixelFormat.Bgra8,
                        BitmapAlphaMode.Straight,
                        (uint)shot.PixelWidth,
                        (uint)shot.PixelHeight,
                        logicalDpi,
                        logicalDpi,
                        pixels.ToArray());

                    await encoder.FlushAsync();
                }

                //ret = ResourceManager.Interface.Viewer.ScreenshotSavedSuccessfully + "\n" + file.Path;
            }
            catch (Exception ex)
            {
                //Debug.WriteLine("ScreenshotMaker -> can't save element '{0}' to picture!\nMsg: '{1}'", element, ex.Message);
                //ret = ResourceManager.Interface.Viewer.ScreenshotNotSaved;
            }

            return ret;
        }
    }
}
