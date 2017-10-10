namespace LaserwarTest.Commons.Observables
{
    /// <summary>
    /// Объект, служащий для представления сведений о внутреннем объекте в UI-интерфейсе
    /// </summary>
    /// <typeparam name="TObject">Тип представляемого объекта</typeparam>
    public class ObservablePresenter<TObject> : ObservableObject
    {
        string _displayText;

        /// <summary>
        /// Внутренний представляемый объект
        /// </summary>
        public TObject UnderlyingObject { get; }

        /// <summary>
        /// Получает представление объекта в виде строки
        /// </summary>
        public string DisplayText
        {
            protected set { SetProperty(ref _displayText, value); }
            get { return _displayText; }
        }

        public ObservablePresenter(TObject obj) : this(obj, "<NULL>") { }
        public ObservablePresenter(TObject obj, string displayName)
        {
            UnderlyingObject = obj;
            _displayText = displayName;
        }

        /// <summary>
        /// При переопределении вызывает обновление сведений об объекте
        /// </summary>
        public virtual void Update() { }

        public static implicit operator TObject(ObservablePresenter<TObject> obj) => obj.UnderlyingObject;
    }
}
