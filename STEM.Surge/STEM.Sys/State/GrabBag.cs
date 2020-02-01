using System;
using System.Collections.Generic;
using System.Linq;

namespace STEM.Sys.State
{
    public class GrabBag<T>
    {
        public enum GrabStyle { RoundRobin, Random }

        public string ID { get; private set; }

        List<T> _Items = null;
        List<T> _Suspended = new List<T>();

        public GrabBag(List<T> items, string id)
        {
            ID = id;
            _Items = items;
        }

        int _RRIndex = 0;
        Random _Rnd = new Random();

        public T Next(GrabStyle style = GrabStyle.RoundRobin)
        {
            lock (_Items)
            {
                T i = default(T);

                if (style == GrabStyle.RoundRobin)
                {
                    if (_RRIndex >= _Items.Count)
                        _RRIndex = 0;

                    if (_RRIndex < _Items.Count)
                        i = _Items[_RRIndex++];
                }
                else
                {
                    if (_Items.Count > 1)
                        i = _Items.OrderBy(x => _Rnd.Next()).FirstOrDefault();
                    else if (_Items.Count == 1)
                        i = _Items[0];
                }

                return i;
            }
        }

        public void Suspend(T item)
        {
            lock (_Items)
            {
                if (_Items.Contains(item))
                {
                    if (!_Suspended.Contains(item))
                        _Suspended.Add(item);
                    
                    _Items.Remove(item);
                }
            }
        }

        public void Resume(T item)
        {
            lock (_Items)
            {
                if (_Suspended.Contains(item))
                {
                    if (!_Items.Contains(item))
                        _Items.Add(item);

                    _Suspended.Remove(item);
                }
            }
        }
    }
}
