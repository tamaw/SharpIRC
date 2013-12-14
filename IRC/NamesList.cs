#region License
// Copyright 2013 Tama Waddell <me@tama.id.au>
// 
// This file is a part of IRC. <https://github.com/tamaw/SharpIRC>
//  
// This source is subject to the Microsoft Public License.
// <http://www.microsoft.com/opensource/licenses.mspx#Ms-PL>
//  All other rights reserved.
#endregion
using System;
using System.Collections;
using System.Collections.Generic;

namespace IRC
{
    public sealed class NamesList : EventArgs, IList<User>
    {
        private readonly List<User> _users;

        public NamesList()
        {
            _users = new List<User>();
        }

        public NamesList(NamesList namesList)
        {
            _users = new List<User>(namesList._users);
        }

        public void Add(User item)
        {
            _users.Add(item);
        }

        public void Clear()
        {
            _users.Clear();
        }

        public bool Contains(User item)
        {
            return _users.Contains(item);
        }

        public void CopyTo(User[] array, int arrayIndex)
        {
            _users.CopyTo(array, arrayIndex);
        }

        IEnumerator<User> IEnumerable<User>.GetEnumerator()
        {
            return _users.GetEnumerator();
        }

        public IEnumerator<User> GetEnumerator()
        {
            return _users.GetEnumerator();
        }

        public int IndexOf(User item)
        {
            return _users.IndexOf(item);
        }

        public void Insert(int index, User item)
        {
            _users.Insert(index, item);
        }

        public bool Remove(User item)
        {
            return _users.Remove(item);
        }

        public int Count { get; private set; }
        public bool IsReadOnly { get; private set; }

        public void RemoveAt(int index)
        {
            _users.RemoveAt(index);
        }

        public User this[int index]
        {
            get { return _users[index]; }
            set { _users[index] = value; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}