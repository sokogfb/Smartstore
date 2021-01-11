﻿using System.Threading.Tasks;
using Smartstore.Core.Data;
using Smartstore.Data;

namespace Smartstore.Core.Catalog.Products
{
    public partial class BackInStockSubscriptionService : IBackInStockSubscriptionService
    {
        private readonly SmartDbContext _db;
        //private readonly IMessageFactory _messageFactory;

        public BackInStockSubscriptionService(
            SmartDbContext db
            /*IMessageFactory messageFactory*/)
        {
            _db = db;
            //_messageFactory = messageFactory
        }

        // TODO: (mg) (core) Complete SendNotificationsToSubscribersAsync (IMessageFactory required).
        public virtual async Task<int> SendNotificationsToSubscribersAsync(Product product)
        {
            Guard.NotNull(product, nameof(product));

            var numberOfMessages = 0;
            var subscriptionQuery = _db.BackInStockSubscriptions.ApplyStandardFilter(0, product.Id, 0);
            var pager = new FastPager<BackInStockSubscription>(subscriptionQuery);

            while ((await pager.ReadNextPageAsync<BackInStockSubscription>()).Out(out var subscriptions))
            {
                //foreach (var subscription in subscriptions)
                //{
                //    // Ensure that customer is registered (simple and fast way).
                //    if (subscription?.Customer?.Email?.IsEmail() ?? false)
                //    {
                //        _messageFactory.SendBackInStockNotification(subscription);
                //        ++numberOfMessages;
                //    }
                //}

                //_db.BackInStockSubscriptions.RemoveRange(subscriptions);

                //await _db.SaveChangesAsync();
            }

            return numberOfMessages;
        }
    }
}