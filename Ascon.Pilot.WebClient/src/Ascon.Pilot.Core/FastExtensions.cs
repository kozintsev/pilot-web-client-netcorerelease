using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable ForCanBeConvertedToForeach

namespace Ascon.Pilot.Core
{
    public static class FastExtensions
    {
        public static bool HasObjectId(this IReadOnlyList<DChild> children, Guid objectId)
        {
            for (int i = 0; i < children.Count; i++)
            {
                var current = children[i];
                if (current.ObjectId == objectId)
                    return true;
            }
            return false;
        }

        public static DFile FileWithId(this IReadOnlyList<DFile> files, Guid id)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var current = files[i];
                if (current.Body.Id == id)
                    return current;
            }
            return null;
        }

        public static DSignature SignatureWithId(this IReadOnlyList<DSignature> signatures, Guid id)
        {
            for (int i = 0; i < signatures.Count; i++)
            {
                var current = signatures[i];
                if (current.Id == id)
                    return current;
            }
            return null;
        }

        public static HashSet<Guid> ToHashSet(this IReadOnlyList<DSignature> signatures)
        {
            var ids = new Guid[signatures.Count];
            for (int i = 0; i < signatures.Count; i++)
            {
                ids[i] = signatures[i].Id;
            }
            return new HashSet<Guid>(ids);
        }

        public static bool HasRelationId(this IReadOnlyList<DRelation> relations, Guid id)
        {
            for (int i = 0; i < relations.Count; i++)
            {
                var current = relations[i];
                if (current.Id == id)
                    return true;
            }
            return false;
        }

        public static bool ListEquals<T>(this IReadOnlyList<T> a, IReadOnlyList<T> b)
        {
            if (a.Count != b.Count)
                return false;

            for (int i = 0; i < a.Count; i++)
            {
                if (!a[i].Equals(b[i]))
                    return false;
            }
            return true;
        }

        public static HashSet<T> FastExcept<T>(this IEnumerable<T> a, IEnumerable<T> b)
        {
            var hashSet = new HashSet<T>(a);
            hashSet.ExceptWith(b);
            return hashSet;
        }

        public static bool HasFileId(this IReadOnlyList<DFile> files, Guid fileId)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var current = files[i];
                if (current.Body.Id == fileId)
                    return true;
            }
            return false;
        }

        public static bool HasFileName(this IReadOnlyList<DFile> files, Func<string, bool> predicate)
        {
            for (int i = 0; i < files.Count; i++)
            {
                var current = files[i];
                if (predicate(current.Name))
                    return true;
            }
            return false;
        }

        public static T FastLast<T>(this IReadOnlyList<T> list)
        {
            return list[list.Count - 1];
        }
    }
}
