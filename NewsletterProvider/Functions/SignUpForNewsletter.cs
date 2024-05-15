using Data.Contexts;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsletterProvider.Models;
using Newtonsoft.Json;

namespace NewsletterProvider.Functions
{
    public class SignUpForNewsletter
    {
        private readonly ILogger<SignUpForNewsletter> _logger;
        private readonly DataContext _dataContext;

        public SignUpForNewsletter(ILogger<SignUpForNewsletter> logger, DataContext dataContext)
        {
            _logger = logger;
            _dataContext = dataContext;
        }
        /// <summary>
        /// Post a Json with headers: ("Api-address", Encoding.UTF8, JsonContent) to this endpoint.
        /// An object of NewsletterRegistrationRequest, i.e. Email, FirstName, LastName
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Function("SignUpForNewsletter")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            // Receive the http request (post only)
            var body = await new StreamReader(req.Body).ReadToEndAsync();

            // Check if the request is containing anything
            if (body != null)
            {
                //"unpack" it
                var nrr = JsonConvert.DeserializeObject<NewsletterRegistrationRequest>(body);

                //Check if the newsletter registration request is correctly formatted
                if (nrr != null && !string.IsNullOrEmpty(nrr.Email) && !string.IsNullOrEmpty(nrr.FirstName) && !string.IsNullOrEmpty(nrr.LastName))
                {

                    var exists = await _dataContext.NewsletterSubscribers.AnyAsync(x => x.Email == nrr.Email);

                    //Check if email already exists in the register
                    if (exists)
                    {
                        return new ConflictResult();
                    }

                    //if not, then create it
                    // create new instance of object
                    var newsletterSubscriber = new NewsletterSubscriberEntity
                    {
                        Email = nrr.Email,
                        FirstName = nrr.FirstName,
                        LastName = nrr.LastName,
                    };

                    try
                    {
                        var result = await _dataContext.NewsletterSubscribers.AddAsync(newsletterSubscriber);
                        await _dataContext.SaveChangesAsync();

                        return new OkResult();
                    }
                    catch (Exception ex) 
                    {
                        return new BadRequestResult(); 
                    }
                }
            }
            return new BadRequestResult();
        }
    }
}
