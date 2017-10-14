using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LaserwarTest.Commons.Observables
{
    public delegate bool GroupSorterSelector<TGroup, TItem>(TGroup group, TItem item);

    /// <summary>
    /// Представляет коллекцию элементов, поддерживающих сортировку,
    /// представляющих собой некоторую группу
    /// </summary>
    /// <typeparam name="TGroup">Тип представляемой группы</typeparam>
    /// <typeparam name="TItem">Коллекция элементов, имеющих отношение к данной группе</typeparam>
    public class GroupSorter<TGroup, TItem> : ObservableObject 
    {
        protected Sorter<TItem> _itemsSorter;
        GroupSorterSelector<TGroup, TItem> _groupSorterSelector;

        /// <summary>
        /// Получает группу, к которой имеют отношение элементы
        /// </summary>
        public TGroup Group { get; }

        public ObservableCollection<TItem> SortedItems => _itemsSorter.SortedItems;

        public GroupSorter(TGroup group, GroupSorterSelector<TGroup, TItem> groupSorterSelector) : this(group, groupSorterSelector, null) { }
        public GroupSorter(TGroup group, GroupSorterSelector<TGroup, TItem> groupSorterSelector, IEnumerable<TItem> collection)
        {
            Group = group;

            _groupSorterSelector = groupSorterSelector;
            _itemsSorter = (collection == null)
                ? new Sorter<TItem>()
                : new Sorter<TItem>(collection.Where(x => _groupSorterSelector(group, x)));
        }

        public void Clear()
        {
            _itemsSorter.Clear();
        }

        public void IsItemOfGroup(TItem item) => _groupSorterSelector(Group, item);

        public void Add(TItem item)
        {
            if (_groupSorterSelector(Group, item))
                _itemsSorter.Add(item);
        }

        public void Remove(TItem item)
        {
            if (_groupSorterSelector(Group, item))
                _itemsSorter.Remove(item);
        }

        public void SortItems(IComparer<TItem> sortOrder)
        {
            _itemsSorter.SortItems(sortOrder);
            OnPropertyChanged(nameof(SortedItems));
        }
    }
}
