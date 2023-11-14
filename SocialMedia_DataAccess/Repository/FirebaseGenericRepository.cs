using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocialMedia_DataAccess.Repository
{
    public class FirebaseGenericRepository<T> : IFirebaseGenericRepository<T> where T : class
    {
        private readonly FirestoreDb _firebaseDb;
        private readonly CollectionReference _collection;
        public FirebaseGenericRepository(FirestoreDb firestoreDb)
        {
            _firebaseDb = firestoreDb;
            _collection = _firebaseDb.Collection(typeof(T).Name);
        }

        public async Task Delete(string id)
        {
            await _collection.Document(id).DeleteAsync();
        }

        public async Task DeleteAll(List<T> objs)
        {
            WriteBatch batch = _firebaseDb.StartBatch();

            foreach (var obj in objs)
            {
                var docRef = _collection.Document(obj.GetType().GetProperty("Id").GetValue(obj).ToString());
                batch.Delete(docRef);
            }

            await batch.CommitAsync();
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            var snapshot = await _collection.GetSnapshotAsync();
            List<T> entities = new List<T>();

            foreach (var document in snapshot.Documents)
            {
                entities.Add(document.ConvertTo<T>());
            }

            return entities;
        }

        public async Task<T> GetById(string id)
        {
            var document = await _collection.Document(id).GetSnapshotAsync();

            if (document.Exists)
            {
                return document.ConvertTo<T>();
            }
            else
            {
                return null;
            }
        }

        public async Task Insert(T entity)
        {
            var document = _collection.Document(entity.GetType().GetProperty("Id").GetValue(entity).ToString());
            await document.SetAsync(entity);
        }

        public async Task Update(T entity)
        {
            var document = _collection.Document(entity.GetType().GetProperty("Id").GetValue(entity).ToString());
            await document.SetAsync(entity, SetOptions.MergeAll);
        }
    }
}
