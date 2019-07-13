using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using LiteDB;

namespace Anilibria.Storage {

	/// <summary>
	/// Entity collection.
	/// </summary>
	public interface IEntityCollection<T> {

		/// <summary>
		/// Count all entites within collection.
		/// </summary>
		int Count ();

		/// <summary>
		/// Count entities with apply filter.
		/// </summary>
		/// <param name="predicate">Predicate.</param>
		int Count ( Expression<Func<T , bool>> predicate );

		/// <summary>
		/// Add new entity.
		/// </summary>
		/// <param name="entity">New entity</param>
		void Add ( T entity );

		/// <summary>
		/// Add range entities.
		/// </summary>
		/// <param name="entities">New entity collection.</param>
		void AddRange ( IEnumerable<T> entities );

		/// <summary>
		/// Add new entity and return id.
		/// </summary>
		/// <param name="entity">New entity</param>
		TId Add<TId> ( T entity );

		/// <summary>
		/// Get by identifier.
		/// </summary>
		/// <typeparam name="TId">Type of identifier.</typeparam>
		/// <param name="id">Identifier.</param>
		/// <param name="includes">Include collection.</param>
		T GetById<TId> ( TId id , params string[] includes );

		/// <summary>
		/// Update existing entity.
		/// </summary>
		/// <param name="entity">Entity that need update.</param>
		void Update ( T entity );

		/// <summary>
		/// Update collection existing entities.
		/// </summary>
		/// <param name="entities">Entit's colection that need update.</param>
		void Update ( IEnumerable<T> entities );

		/// <summary>
		/// Get first entity or default type value.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		T FirstOrDefault ();

		/// <summary>
		/// Get first entity specify filter or default type value.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		T FirstOrDefault ( Expression<Func<T , bool>> predicate );

		/// <summary>
		/// Get first entity specify filter or default type value.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		/// <param name="includes">Include collection.</param>
		T FirstOrDefault ( Expression<Func<T , bool>> predicate , params string[] includes );

		/// <summary>
		/// Find entities.
		/// </summary>
		/// <param name="predicate">Filter conditions.</param>
		/// <param name="take">Number items in response.</param>
		/// <param name="skip">Shifting elements from the beginning of the list.</param>
		/// <typeparam name="T">Type of entity.</typeparam>
		IEnumerable<T> Find ( Expression<Func<T , bool>> predicate , int skip = 0 , int take = int.MaxValue );

		/// <summary>
		/// Find entities.
		/// </summary>
		/// <param name="query">Query.</param>
		/// <param name="take">Number items in response.</param>
		/// <param name="skip">Shifting elements from the beginning of the list.</param>
		/// <typeparam name="T">Type of entity.</typeparam>
		IEnumerable<T> Find ( Query query , int skip = 0 , int take = int.MaxValue );

		/// <summary>
		/// Get all entities in collection.
		/// </summary>
		/// <typeparam name="T">Type of entity.</typeparam>
		IEnumerable<T> All ();

		/// <summary>
		/// Delete by id.
		/// </summary>
		/// <param name="id">Identifier.</param>
		void DeleteById ( long id );

		/// <summary>
		/// Delete by query.
		/// </summary>
		/// <param name="query">Query for select record for delete.</param>
		void Delete ( Query query );

		/// <summary>
		/// Delete by query.
		/// </summary>
		/// <param name="predicate">Predicate for select record for delete.</param>
		void Delete ( Expression<Func<T , bool>> predicate );

		/// <summary>
		/// Include for collection.
		/// </summary>
		/// <param name="includes">Include collection.</param>
		void Include ( params string[] includes );

		/// <summary>
		/// Maximum value by property.
		/// </summary>
		/// <param name="expression">Expression for specify property.</param>
		BsonValue Max ( Expression<Func<T , BsonValue>> expression );

	}

}
