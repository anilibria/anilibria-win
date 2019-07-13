using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Anilibria.Storage;
using LiteDB;

namespace Anilibria.Services.Implementations {

	/// <summary>
	/// Adapter for entity collection based on <see cref="LiteCollection{T}"/> class.
	/// </summary>
	public class LiteDbEntityCollection<T> : IEntityCollection<T> {

		private readonly LiteCollection<T> m_Collection;

		/// <summary>
		/// Create entity collection from LiteDb collection.
		/// </summary>
		/// <param name="collection">LiteDb collection.</param>
		/// <exception cref="ArgumentNullException"></exception>
		public LiteDbEntityCollection ( LiteCollection<T> collection ) {
			m_Collection = collection ?? throw new ArgumentNullException ( nameof ( collection ) );
		}

		/// <summary>
		/// Count all entities within collection.
		/// </summary>
		/// <returns></returns>
		public int Count () {
			return m_Collection.Count ();
		}

		/// <summary>
		/// Count entities with apply filter.
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		public int Count ( Expression<Func<T , bool>> predicate ) {
			return m_Collection.Count ( predicate );
		}

		/// <summary>
		/// Add new entity.
		/// </summary>
		/// <param name="entity">New entity</param>
		public void Add ( T entity ) {
			m_Collection.Insert ( entity );
		}

		/// <summary>
		/// Add range entities.
		/// </summary>
		/// <param name="entities">New entity collection.</param>
		public void AddRange ( IEnumerable<T> entities ) {
			m_Collection.Insert ( entities );
		}

		/// <summary>
		/// Add new entity and return id.
		/// </summary>
		/// <param name="entity">New entity</param>
		public TId Add<TId> ( T entity ) {
			var value = m_Collection.Insert ( entity );

			return (TId) value.RawValue;
		}

		/// <summary>
		/// Get by identifier.
		/// </summary>
		/// <typeparam name="TId">Type of identifier.</typeparam>
		/// <param name="includes">Include collection.</param>
		/// <param name="id">Identifier.</param>
		public T GetById<TId> ( TId id , params string[] includes ) {
			var collection = m_Collection;
			foreach ( var include in includes ) {
				collection = collection.Include ( include );
			}

			return collection.FindById ( new BsonValue ( id ) );
		}

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Entity that need update.</param>
		public void Update ( T entity ) {
			m_Collection.Update ( entity );
		}

		/// <summary>
		/// Update collection existing entities.
		/// </summary>
		/// <param name="entities">Entit's colection that need update.</param>
		public void Update ( IEnumerable<T> entities ) {
			m_Collection.Update ( entities );
		}

		/// <summary>
		/// Get first entity or default type value.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		public T FirstOrDefault () {
			return m_Collection.FindOne ( a => true );
		}

		/// <summary>
		/// Get first entity specify filter or default type value.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		public T FirstOrDefault ( Expression<Func<T , bool>> predicate ) {
			return m_Collection.FindOne ( predicate );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="predicate"></param>
		/// <param name="includes"></param>
		/// <returns></returns>
		public T FirstOrDefault ( Expression<Func<T , bool>> predicate , params string[] includes ) {
			var collection = m_Collection;
			foreach ( var include in includes ) {
				collection = collection.Include ( include );
			}

			return m_Collection.FindOne ( predicate );
		}

		/// <summary>
		/// Get first entity or default type value.
		/// </summary>
		/// <param name="predicate">Predicate for filter collection.</param>
		/// <param name="take">Number items in response.</param>
		/// <param name="skip">Shifting elements from the beginning of the list.</param>
		/// <typeparam name="T">Type of entity.</typeparam>
		public IEnumerable<T> Find ( Expression<Func<T , bool>> predicate , int skip = 0 , int take = int.MaxValue ) {
			if ( predicate == null ) return m_Collection.FindAll ();

			return m_Collection.Find ( predicate , skip , take );
		}

		/// <summary>
		/// Find entities.
		/// </summary>
		/// <param name="query">Query for filter collection.</param>
		/// <param name="skip">Offset at begin result.</param>
		/// <param name="take">Limit the result of the records.</param>
		/// <returns></returns>
		public IEnumerable<T> Find ( Query query , int skip = 0 , int take = int.MaxValue ) {
			if ( query == null ) return m_Collection.FindAll ();

			return m_Collection.Find ( query , skip , take );
		}

		/// <summary>
		/// Delete by id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void DeleteById ( long id ) {
			m_Collection.Delete ( id );
		}

		/// <summary>
		/// Delete by query.
		/// </summary>
		/// <param name="id">Identifier.</param>
		public void Delete ( Query query ) {
			m_Collection.Delete ( query );
		}

		/// <summary>
		/// Delete by query.
		/// </summary>
		/// <param name="predicate">Predicate for select record for delete.</param>
		public void Delete ( Expression<Func<T , bool>> predicate ) {
			m_Collection.Delete ( predicate );
		}

		/// <summary>
		/// Include for collection.
		/// </summary>
		/// <param name="includes">Include collection.</param>
		public void Include ( params string[] includes ) {
			var collection = m_Collection;
			foreach ( var include in includes ) {
				collection = collection.Include ( include );
			}
		}

		/// <summary>
		/// Maximum by property.
		/// </summary>
		/// <param name="expression">Expression for specify mixumum field.</param>
		/// <returns>Maximum value.</returns>
		public BsonValue Max ( Expression<Func<T , BsonValue>> expression ) {
			return m_Collection.Max ( expression );
		}

		/// <summary>
		/// Get all entities in collection.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		public IEnumerable<T> All () => m_Collection.FindAll ();

	}

}
