using Data.Contexts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsletterProvider.Models;
using Newtonsoft.Json;

namespace NewsletterProvider.Functions
{
    public class RemoveFromNewsletter
    {
        private readonly ILogger<RemoveFromNewsletter> _logger;
        private readonly DataContext _dataContext;

        public RemoveFromNewsletter(ILogger<RemoveFromNewsletter> logger, DataContext datacontext)
        {
            _logger = logger;
            _dataContext = datacontext;
        }

        [Function("RemoveFromNewsletter")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            var body = await new StreamReader(req.Body).ReadToEndAsync();

            if (body != null)
            {
                var nrr = JsonConvert.DeserializeObject<NewsletterRemovalRequest>(body);

                if (nrr.Email != null)
                {
                    try
                    {
                        var subscriberToRemove = await _dataContext.NewsletterSubscribers.FirstOrDefaultAsync(x => x.Email == nrr.Email);
                        var result = _dataContext.NewsletterSubscribers.Remove(subscriberToRemove!);
                        await _dataContext.SaveChangesAsync();

                        return new OkResult();
                    }
                    catch (Exception ex)
                    {
                        return new NotFoundResult();
                    }
                }
            }
            return new BadRequestResult();
        }
    }
}
