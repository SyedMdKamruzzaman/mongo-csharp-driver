﻿/* Copyright 2010-2014 MongoDB Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver.Core.Misc;

namespace MongoDB.Driver
{
    /// <summary>
    /// Extension methods for <see cref="IFindFluent{TDocument, TProjection}"/>
    /// </summary>
    public static class IFindFluentExtensions
    {
        /// <summary>
        /// Projects the result.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="projection">The projection.</param>
        /// <returns>The fluent find interface.</returns>
        public static IFindFluent<TDocument, BsonDocument> Project<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> find, ProjectionDefinition<TDocument, BsonDocument> projection)
        {
            Ensure.IsNotNull(find, "find");
            Ensure.IsNotNull(projection, "projection");

            return find.Project<BsonDocument>(projection);
        }

        /// <summary>
        /// Projects the result.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <typeparam name="TNewProjection">The type of the new projection.</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="projection">The projection.</param>
        /// <returns>The fluent find interface.</returns>
        public static IFindFluent<TDocument, TNewProjection> Project<TDocument, TProjection, TNewProjection>(this IFindFluent<TDocument, TProjection> find, Expression<Func<TDocument, TNewProjection>> projection)
        {
            Ensure.IsNotNull(find, "find");
            Ensure.IsNotNull(projection, "projection");

            return find.Project<TNewProjection>(new FindExpressionProjectionDefinition<TDocument, TNewProjection>(projection));
        }

        /// <summary>
        /// Sorts the results by an ascending field.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="field">The field.</param>
        /// <returns>The fluent find interface.</returns>
        public static IOrderedFindFluent<TDocument, TProjection> SortBy<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> find, Expression<Func<TDocument, object>> field)
        {
            Ensure.IsNotNull(find, "find");
            Ensure.IsNotNull(field, "field");

            // We require an implementation of IFindFluent<TDocument, TProjection> 
            // to also implement IOrderedFindFluent<TDocument, TProjection>
            return (IOrderedFindFluent<TDocument, TProjection>)find.Sort(
                new DirectionalSortDefinition<TDocument>(new ExpressionFieldDefinition<TDocument>(field), SortDirection.Ascending));
        }

        /// <summary>
        /// Sorts the results by a descending field.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="field">The field.</param>
        /// <returns>The fluent find interface.</returns>
        public static IOrderedFindFluent<TDocument, TProjection> SortByDescending<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> find, Expression<Func<TDocument, object>> field)
        {
            Ensure.IsNotNull(find, "find");
            Ensure.IsNotNull(field, "field");

            // We require an implementation of IFindFluent<TDocument, TProjection> 
            // to also implement IOrderedFindFluent<TDocument, TProjection>
            return (IOrderedFindFluent<TDocument, TProjection>)find.Sort(
                new DirectionalSortDefinition<TDocument>(new ExpressionFieldDefinition<TDocument>(field), SortDirection.Descending));
        }

        /// <summary>
        /// Adds an ascending field to the existing sort.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="field">The field.</param>
        /// <returns>The fluent find interface.</returns>
        public static IOrderedFindFluent<TDocument, TProjection> ThenBy<TDocument, TProjection>(this IOrderedFindFluent<TDocument, TProjection> find, Expression<Func<TDocument, object>> field)
        {
            Ensure.IsNotNull(find, "find");
            Ensure.IsNotNull(field, "field");

            find.Options.Sort = new SortDefinitionBuilder<TDocument>().Combine(
                find.Options.Sort,
                new DirectionalSortDefinition<TDocument>(new ExpressionFieldDefinition<TDocument>(field), SortDirection.Ascending));

            return find;
        }

        /// <summary>
        /// Adds a descending field to the existing sort.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="field">The field.</param>
        /// <returns>The fluent find interface.</returns>
        public static IOrderedFindFluent<TDocument, TProjection> ThenByDescending<TDocument, TProjection>(this IOrderedFindFluent<TDocument, TProjection> find, Expression<Func<TDocument, object>> field)
        {
            Ensure.IsNotNull(find, "find");
            Ensure.IsNotNull(field, "field");

            find.Options.Sort = new SortDefinitionBuilder<TDocument>().Combine(
                find.Options.Sort,
                new DirectionalSortDefinition<TDocument>(new ExpressionFieldDefinition<TDocument>(field), SortDirection.Descending));

            return find;
        }

        /// <summary>
        /// Get the first result.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the first result.</returns>
        public async static Task<TProjection> FirstAsync<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> find, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.IsNotNull(find, "find");

            using (var cursor = await find.Limit(1).ToCursorAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    return cursor.Current.First();
                }
                else
                {
                    throw new InvalidOperationException("The source sequence is empty.");
                }
            }
        }

        /// <summary>
        /// Get the first result or null.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the first result or null.</returns>
        public async static Task<TProjection> FirstOrDefaultAsync<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> find, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.IsNotNull(find, "find");

            using (var cursor = await find.Limit(1).ToCursorAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    return cursor.Current.FirstOrDefault();
                }
                else
                {
                    return default(TProjection);
                }
            }
        }

        /// <summary>
        /// Gets a single result.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the single result.</returns>
        public async static Task<TProjection> SingleAsync<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> find, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.IsNotNull(find, "find");

            using (var cursor = await find.Limit(2).ToCursorAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    return cursor.Current.Single();
                }
                else
                {
                    throw new InvalidOperationException("The source sequence is empty.");
                }
            }
        }

        /// <summary>
        /// Gets a single result or null.
        /// </summary>
        /// <typeparam name="TDocument">The type of the document.</typeparam>
        /// <typeparam name="TProjection">The type of the projection (same as TDocument if there is no projection).</typeparam>
        /// <param name="find">The fluent find.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A Task whose result is the single result or null.</returns>
        public async static Task<TProjection> SingleOrDefaultAsync<TDocument, TProjection>(this IFindFluent<TDocument, TProjection> find, CancellationToken cancellationToken = default(CancellationToken))
        {
            Ensure.IsNotNull(find, "find");

            using (var cursor = await find.Limit(2).ToCursorAsync(cancellationToken).ConfigureAwait(false))
            {
                if (await cursor.MoveNextAsync(cancellationToken).ConfigureAwait(false))
                {
                    return cursor.Current.SingleOrDefault();
                }
                else
                {
                    return default(TProjection);
                }
            }
        }
    }
}
