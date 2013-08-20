using System;
using System.Collections.Generic;
using System.Linq;
using Lidgren.Network;

namespace Programe.Server
{
    public class SessionManager
    {
        private Dictionary<long, Session> sessions;

        public SessionManager()
        {
            sessions = new Dictionary<long, Session>();
        }

        /// <summary>
        /// Create a session from a connection.
        /// </summary>
        public Session Create(NetConnection connection)
        {
            lock (sessions)
            {
                var session = new Session(connection);
                sessions[connection.RemoteUniqueIdentifier] = session;
                return session;
            }
        }

        /// <summary>
        /// Destroy a session.
        /// </summary>
        /// <param name="id"></param>
        public void Destroy(long id)
        {
            lock (sessions)
                sessions.Remove(id);
        }

        /// <summary>
        /// Get a session.
        /// </summary>
        public Session Get(long id)
        {
            lock (sessions)
                return sessions[id];
        }

        /// <summary>
        /// Destroy all sessions that have been disconnected.
        /// </summary>
        public void Clean()
        {
            lock (sessions)
            {
                var remove = sessions.Where(kv => kv.Value.Connection.Status == NetConnectionStatus.Disconnected).ToList();
                foreach (var r in remove)
                {
                    sessions.Remove(r.Key);
                }
            }
        }

        /// <summary>
        /// Disconnect and destroy all sessions with a username.
        /// </summary>
        public void Kick(string username)
        {
            lock (sessions)
            {
                var remove = sessions.Where(kv => kv.Value.Account != null && kv.Value.Account.Username == username).ToList();
                foreach (var r in remove)
                {
                    r.Value.Connection.Disconnect("No multiple logins");
                    sessions.Remove(r.Key);
                }
            }
        }
    }
}
 