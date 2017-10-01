namespace LaserwarTest.Core.Data.DB
{
    /// <summary>
    /// Представляет отдельную сущность в базе данных
    /// </summary>
    public interface ISQLiteDBEntity
    {
    }

    /// <summary>
    /// Представляет отдельную сущность в базе данных, идентифицирующаяся через указанный первичный ключ
    /// </summary>
    /// <typeparam name="TPrimaryKey">Тип первичного ключа</typeparam>
    public interface ISQLiteDBEntity<TPrimaryKey> : ISQLiteDBEntity
    {
        TPrimaryKey ID { set; get; }
    }
}
