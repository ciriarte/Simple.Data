﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Simple.Data.UnitTest
{
    [TestFixture]
    public class SimpleQueryTest
    {
        [Test]
        public void WhereShouldSetCriteria()
        {
            var query = new SimpleQuery(null, "foo");
            var criteria = new SimpleExpression(1, 1, SimpleExpressionType.Equal);
            query = query.Where(criteria);
            Assert.AreSame(criteria, query.Criteria);
        }

        [Test]
        public void SkipShouldSetSkipCount()
        {
            var query = new SimpleQuery(null, "foo");
            query = query.Skip(42);
            Assert.AreEqual(42, query.SkipCount);
        }

        [Test]
        public void TakeShouldSetTakeCount()
        {
            var query = new SimpleQuery(null, "foo");
            query = query.Take(42);
            Assert.AreEqual(42, query.TakeCount);
        }

        [Test]
        public void OrderByShouldSetOrderAscending()
        {
            var query = new SimpleQuery(null, "foo");
            query = query.OrderBy(new ObjectReference("bar"));
            Assert.AreEqual("bar", query.Order.Single().Reference.GetName());
            Assert.AreEqual(OrderByDirection.Ascending, query.Order.Single().Direction);
        }

        [Test]
        public void OrderByBarShouldSetOrderAscending()
        {
            dynamic query = new SimpleQuery(null, "foo");
            SimpleQuery actual = query.OrderByBar();
            Assert.AreEqual("bar", actual.Order.Single().Reference.GetName().ToLowerInvariant());
            Assert.AreEqual(OrderByDirection.Ascending, actual.Order.Single().Direction);
        }

        [Test]
        public void OrderByBarThenByQuuxShouldSetOrderAscending()
        {
            dynamic query = new SimpleQuery(null, "foo");
            SimpleQuery actual = query.OrderByBar().ThenByQuux();
            Assert.AreEqual("bar", actual.Order.First().Reference.GetName().ToLowerInvariant());
            Assert.AreEqual("quux", actual.Order.Skip(1).First().Reference.GetName().ToLowerInvariant());
            Assert.AreEqual(OrderByDirection.Ascending, actual.Order.First().Direction);
            Assert.AreEqual(OrderByDirection.Ascending, actual.Order.Skip(1).First().Direction);
        }
        
        [Test]
        public void ThenByShouldModifyOrderAscending()
        {
            var query = new SimpleQuery(null, "foo");
            query = query.OrderBy(new ObjectReference("bar")).ThenBy(new ObjectReference("quux"));
            var actual = query.Order.ToArray();
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("bar", actual[0].Reference.GetName());
            Assert.AreEqual(OrderByDirection.Ascending, actual[0].Direction);
            Assert.AreEqual("quux", actual[1].Reference.GetName());
            Assert.AreEqual(OrderByDirection.Ascending, actual[1].Direction);
        }

        [Test]
        public void OrderByDescendingShouldSetOrderDescending()
        {
            var query = new SimpleQuery(null, "foo");
            query = query.OrderByDescending(new ObjectReference("bar"));
            Assert.AreEqual("bar", query.Order.Single().Reference.GetName());
            Assert.AreEqual(OrderByDirection.Descending, query.Order.Single().Direction);
        }

        [Test]
        public void OrderByBarDescendingShouldSetOrderDescending()
        {
            dynamic query = new SimpleQuery(null, "foo");
            SimpleQuery actual = query.OrderByBarDescending();
            Assert.AreEqual("bar", actual.Order.Single().Reference.GetName().ToLowerInvariant());
            Assert.AreEqual(OrderByDirection.Descending, actual.Order.Single().Direction);
        }

        [Test]
        public void ThenByDescendingShouldModifyOrderAscending()
        {
            var query = new SimpleQuery(null, "foo");
            query = query.OrderBy(new ObjectReference("bar")).ThenByDescending(new ObjectReference("quux"));
            var actual = query.Order.ToArray();
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual("bar", actual[0].Reference.GetName());
            Assert.AreEqual(OrderByDirection.Ascending, actual[0].Direction);
            Assert.AreEqual("quux", actual[1].Reference.GetName());
            Assert.AreEqual(OrderByDirection.Descending, actual[1].Direction);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThenByWithoutOrderByShouldThrow()
        {
            new SimpleQuery(null, "foo").ThenBy(new ObjectReference("bar"));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void ThenByDescendingWithoutOrderByShouldThrow()
        {
            new SimpleQuery(null, "foo").ThenByDescending(new ObjectReference("bar"));
        }

        [Test]
        public void JoinShouldCreateExpression()
        {
            dynamic q = new SimpleQuery(null, "foo");
            q = q.Join(new ObjectReference("bar"), foo_id: new ObjectReference("id", new ObjectReference("foo")));
            var query = (SimpleQuery) q;
            Assert.AreEqual(1, query.Joins.Count());
            var join = query.Joins.Single();
            Assert.AreEqual("bar", join.Table.GetName());
        }
    }
}
