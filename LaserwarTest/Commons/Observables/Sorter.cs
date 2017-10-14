using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace LaserwarTest.Commons.Observables
{
    /// <summary>
    /// Представляет обертку для коллекции, поддерживающую сортировку элементов
    /// </summary>
    /// <typeparam name="TItem">Тип элементов</typeparam>
    public class Sorter<TItem> : ObservableObject
    {
        ObservableCollection<TItem> _underlyingCollection;
        ObservableCollection<TItem> _sortedItems;

        IComparer<TItem> _sortOrder;

        public Sorter()
        {
            InitCollection(null);
        }

        public Sorter(IEnumerable<TItem> collection)
        {
            InitCollection(collection);
        }

        protected void InitCollection(IEnumerable<TItem> collection)
        {
            _underlyingCollection = (collection == null)
                ? new ObservableCollection<TItem>()
                : new ObservableCollection<TItem>(collection);

            _underlyingCollection.CollectionChanged += (s, e) => { HandleCollectionChanged(e); };

            Default();
        }

        /// <summary>
        /// Получает общую коллекцию всех сортируемых элементов
        /// </summary>
        protected ObservableCollection<TItem> UnderlyingCollection { get => _underlyingCollection; }

        /// <summary>
        /// Получает коллекцию отсортированных элементов
        /// </summary>
        public ObservableCollection<TItem> SortedItems
        {
            protected set => SetProperty(ref _sortedItems, value);
            get => _sortedItems;
        }

        public void Clear()
        {
            _underlyingCollection.Clear();
            _sortedItems?.Clear();
        }

        public void Add(TItem item) => _underlyingCollection.Add(item);
        public void Remove(TItem item) => _underlyingCollection.Remove(item);

        /// <summary>
        /// Отображает коллекцию элементов в ее изначальном порядке
        /// </summary>
        public void Default() => SortItems(null);
        /// <summary>
        /// Сортирует коллекцию с использованием указанного компаратора
        /// </summary>
        /// <param name="sortOrder"></param>
        public void SortItems(IComparer<TItem> sortOrder)
        {
            _sortOrder = sortOrder;
            if (sortOrder == null)
            {
                SortedItems = new ObservableCollection<TItem>(_underlyingCollection);
                return;
            }

            var temp = _underlyingCollection.OrderBy(i => i, _sortOrder);

            SortedItems = new ObservableCollection<TItem>(temp);
        }

        private void HandleCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                TItem item = (TItem)(e.NewItems[0]);

                if (_sortOrder == null)
                    _sortedItems.Add(item);
                else
                {
                    int index = _sortedItems.ToList().BinarySearch(item, _sortOrder);
                    if (index < 0) _sortedItems.Insert(~index, item);
                    else _sortedItems.Insert(index, item);
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                var item = (TItem)(e.OldItems[0]);
                var targetIndex = _sortedItems.IndexOf(item);

                _sortedItems.RemoveAt(targetIndex);
            }
        }
    }
}
