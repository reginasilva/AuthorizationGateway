using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AuthorizationGateway.Api.Pages
{
    public class TransactionsModel : PageModel
    {
        private readonly IHttpClientFactory _client;

        public List<TransactionResponse> Transactions { get; set; } = new();

        [BindProperty]
        public string? SearchId { get; set; }

        public TransactionsModel(IHttpClientFactory factory)
        {
            _client = factory;
        }

        public async Task OnGetAsync()
        {
            await LoadTransactions();
        }

        public async Task OnPostAsync()
        {
            if (!string.IsNullOrWhiteSpace(SearchId) && Guid.TryParse(SearchId, out var id))
            {
                var client = _client.CreateClient("api");
                var result = await client.GetFromJsonAsync<TransactionResponse>($"transactions/{id}");
                if (result != null)
                    Transactions = new List<TransactionResponse> { result };
            }
            else
            {
                await LoadTransactions();
            }
        }

        private async Task LoadTransactions()
        {
            var client = _client.CreateClient("api");
            var result = await client.GetFromJsonAsync<List<TransactionResponse>>("transactions");

            if (result != null)
            {
                Transactions = result;
            }
        }

        public class TransactionResponse
        {
            public Guid TransactionId { get; set; }

            public string Status { get; set; } = string.Empty;

            public string? MaskedPan { get; set; }

            public string? MaskedTrack2 { get; set; }

            public DateTime CreatedAtUtc { get; set; }

            public DateTime AuthorizedAtUtc { get; set; }

            public string Reason { get; set; } = string.Empty;
        }
    }
}
