namespace FYP.Models.Abstractions
{
    public interface IDataSource
    {
        string GetName();
        void Update(INamedEntity source);
    }
}