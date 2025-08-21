// StajTakipUygulaması.Application/Interfaces/IEmailSender.cs
using System.Threading;
using System.Threading.Tasks;

namespace StajTakipUygulaması.Application.Interfaces
{
    public interface IEmailSender
    {
        Task SendAsync(string to, string subject, string bodyHtml, CancellationToken ct = default);
    }
}
