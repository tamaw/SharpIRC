#region License
// Copyright 2013 Tama Waddell <me@tama.id.au>
// 
// This file is a part of SharpIRC. <https://github.com/tamaw/SharpIRC>
//  
// This source is subject to the Microsoft Public License.
// <http://www.microsoft.com/opensource/licenses.mspx#Ms-PL>
//  All other rights reserved.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IRC;
using SharpIRC.Annotations;

namespace SharpIRC.Views
{
    public interface IIrcTabItemModel
    {
        string Header { get; }
        Type Type { get; }
        void Message(string message);
        void Clear();
    }
}
