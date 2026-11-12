namespace SajamKnjiga.Core.Interfaces;


public interface IDao<T> where T : class, IEntitet
{
    List<T> GetAll();
    T? GetById(int id);
    T Add(T entitet);
    T? Update(T entitet);
    bool Delete(int id);
    void Save();
}
