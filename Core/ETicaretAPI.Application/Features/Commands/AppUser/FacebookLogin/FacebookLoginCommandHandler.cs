using ETicaretAPI.Application.Abstractions.Token;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Features.Commands.AppUser.FacebookLogin
{
    public class FacebookLoginCommandHandler : IRequestHandler<FacebookLoginCommandRequest, FacebookLoginCommandResponse>
    {
        readonly UserManager<Domain.Entities.Identity.AppUser> _userManager;
        readonly ITokenHandler _tokenHandler;
        readonly HttpClient _httpClient;

        public FacebookLoginCommandHandler(UserManager<Domain.Entities.Identity.AppUser> userManager, ITokenHandler tokenHandler,
            IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _tokenHandler = tokenHandler;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<FacebookLoginCommandResponse> Handle(FacebookLoginCommandRequest request, CancellationToken cancellationToken)
        {
            string accessTokenResponse = await _httpClient.GetStringAsync($"https://graph.facebook.com/oauth/access_token?client_id=376143411471617&client_secret=136c7e4a727672eea930efd22ab1dc5e&grant_type=client_credentials");
            return new()
            {

            };
        }
    }
}
