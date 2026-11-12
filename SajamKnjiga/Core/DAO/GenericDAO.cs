using SajamKnjiga.Core.Interfaces;
using SajamKnjiga.Core.Serialization;
using SajamKnjiga.Core.Utils;

namespace SajamKnjiga.Core.DAO;

public class GenericDAO<T> : IDao<T> where T : class, IEntitet, ISerializable, new()
{
    private readonly string _filePath;
    private readonly Serializer<T> _serializer = new Serializer<T>();
    protected readonly List<T> _entiteti;

    public GenericDAO(string fileName)
    {
        _filePath = DataPaths.GetFilePath(fileName);
        _entiteti = _serializer.FromCSV(_filePath);
    }

    public List<T> GetAll()
    {
        return _entiteti;
    }

    public T? GetById(int id)
    {
        return _entiteti.FirstOrDefault(e => e.Id == id);
    }

    public T Add(T entitet)
    {
        entitet.Id = GenerisiId();
        _entiteti.Add(entitet);
        Save();
        return entitet;
    }

    public T? Update(T entitet)
    {
        int index = _entiteti.FindIndex(e => e.Id == entitet.Id);
        if (index < 0)
        {
            return null;
        }

        _entiteti[index] = entitet;
        Save();
        return entitet;
    }

    public bool Delete(int id)
    {
        T? entitet = GetById(id);
        if (entitet == null)
        {
            return false;
        }

        _entiteti.Remove(entitet);
        Save();
        return true;
    }

    public void Save()
    {
        _serializer.ToCSV(_filePath, _entiteti);
    }

    private int GenerisiId()
    {
        return _entiteti.Count == 0 ? 1 : _entiteti.Max(e => e.Id) + 1;
    }
}
