using System.Collections;

namespace JwtViewer.ViewModels.Core;

public class JwtArray : IJwtNode, ICollection<IJwtNode>
{
    private readonly List<IJwtNode> _list = [];
    
    public int Position { get; set; }
    public int JsonStartPosition { get; set; }
    public int JsonEndPosition { get; set; }

    public IEnumerator<IJwtNode> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    public void Add(IJwtNode item) => _list.Add(item);
    public void Clear() => _list.Clear();
    public bool Contains(IJwtNode item) => _list.Contains(item);
    public void CopyTo(IJwtNode[] array, int arrayIndex) => _list.CopyTo(array, arrayIndex);
    public bool Remove(IJwtNode item) => _list.Remove(item);
    public int Count => _list.Count;
    public bool IsReadOnly => false;
}