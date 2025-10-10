using M3U8.NET.Exceptions;
using M3U8.NET.Models;

namespace M3U8.NET.Validators;

public static class M3U8Validator
{
    public static void Validate(Playlist playlist)
    {
        if (playlist is null)
        {
            throw new InvalidM3U8FormatException("Playlist cannot be null.");
        }

        if (playlist.Segments is null || playlist.Segments.Count == 0)
        {
            throw new InvalidM3U8FormatException("Playlist must contain at least one segment.");
        }

        foreach (var segment in playlist.Segments)
        {
            if (segment.Duration < -1)
            {
                throw new InvalidM3U8FormatException($"Invalid duration: {segment.Duration}. Must be -1 or positive.");
            }

            if (string.IsNullOrWhiteSpace(segment.Title))
            {
                throw new InvalidM3U8FormatException("Segment title cannot be empty.");
            }

            if (string.IsNullOrWhiteSpace(segment.Uri) || !Uri.TryCreate(segment.Uri, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new InvalidM3U8FormatException($"Invalid or insecure URI: {segment.Uri}. Must be absolute http/https.");
            }

            if (segment.Attributes.TryGetValue("tvg-logo", out var logo) && !string.IsNullOrEmpty(logo))
            {
                if (!Uri.TryCreate(logo, UriKind.Absolute, out var logoUri) ||
                    (logoUri.Scheme != Uri.UriSchemeHttp && logoUri.Scheme != Uri.UriSchemeHttps))
                {
                    throw new InvalidM3U8FormatException($"Invalid tvg-logo URI: {logo}.");
                }
            }
        }
    }
}
