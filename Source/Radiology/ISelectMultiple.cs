using System.Collections.Generic;

namespace Radiology;

internal interface ISelectMultiple<T>
{
    IEnumerable<T> All();

    string Label(T obj);
    bool IsSelected(T obj);
    void Select(T obj);
    void Unselect(T obj);
}