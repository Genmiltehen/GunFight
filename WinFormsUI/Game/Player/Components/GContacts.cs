using Box2D.NET;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinFormsUI.Game.Box2D;
using XEngine.Core.Base;
using XEngine.Core.Box2DCompat;
using static XEngine.Core.Box2DCompat.B2Helpers;

namespace WinFormsUI.Game.Player.Components
{
    public class GContacts : GameComponent
    {
        private readonly Dictionary<ContactFlags, int> _contacts = [];
        private readonly Dictionary<ContactFlags, HashSet<B2ShapeId>> _contactsSet = [];

        public GContacts Register(ContactFlags flags, bool isTracked = false)
        {
            _contacts[flags] = 0;
            if (isTracked) _contactsSet[flags] = [];
            return this;
        }

        public bool Has(ContactFlags type) => _contacts.TryGetValue(type, out var contact) && contact > 0;
        public HashSet<B2ShapeId> Get(ContactFlags type) => _contactsSet.TryGetValue(type, out var contact) ? contact : [];

        public void AddContacts(B2ShapeId shapeid)
        {
            foreach (var contactId in _contacts.Keys.Where(cid => CheckFlag(shapeid, (ulong)cid)))
            {
                _contacts[contactId]++;
                if (_contactsSet.TryGetValue(contactId, out var contactSet))
                    contactSet.Add(shapeid);
            }
        }

        public void RemoveContacts(B2ShapeId shapeid)
        {
            foreach (var contactId in _contacts.Keys.Where(cid => CheckFlag(shapeid, (ulong)cid)))
            {
                _contacts[contactId]--;
                if (_contactsSet.TryGetValue(contactId, out var contactSet))
                    contactSet.Remove(shapeid);
            }
        }

        public static void ContactParseAdd(ContactWrapper ev)
        {
            if (ev.EntityA?.TryGet<GContacts>(out var c) == true)
            {
                
            }
            ev.EntityA?.Get<GContacts>()?.AddContacts(ev.ShapeIdB);
            ev.EntityB?.Get<GContacts>()?.AddContacts(ev.ShapeIdA);
        }

        public static void ContactParseRemove(ContactWrapper ev)
        {
            ev.EntityA?.Get<GContacts>()?.RemoveContacts(ev.ShapeIdB);
            ev.EntityB?.Get<GContacts>()?.RemoveContacts(ev.ShapeIdA);
        }
    }
}
