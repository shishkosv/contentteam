using ContentPipeline.Core;

namespace ContentPipeline.Adapters.Telegram;

public sealed class TelegramPublisherStub : ITelegramPublisher
{
    public Task<PublishReceipt> PublishAsync(ArtifactRecord artifact, PublishTarget target, CancellationToken cancellationToken = default)
    {
        var receipt = new PublishReceipt(
            ArtifactId: artifact.ArtifactId,
            Platform: target.Platform,
            AccountId: target.AccountId,
            Destination: target.Destination,
            MessageId: Guid.NewGuid().ToString("N"),
            PublishedAt: DateTimeOffset.UtcNow,
            Status: "published");

        return Task.FromResult(receipt);
    }
}
