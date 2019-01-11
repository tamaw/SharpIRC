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

namespace SharpIRC.ViewModel
{
    public class MessageEntry : PropertyChangedBase
    {
        public DateTime DateTime { get; set; }
        public string User { get; set; }
        public string Message { get; set; }
    }

}
