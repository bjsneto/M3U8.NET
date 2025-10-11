# M3U8.NET

[![NuGet Version](https://img.shields.io/nuget/v/M3U8.NET.svg?style=for-the-badge)](https://www.nuget.org/packages/M3U8.NET)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg?style=for-the-badge)](LICENSE)

<br />
<div align="center">
<h3 align="center">M3U8.NET</h3>

<p align="center">
A lightweight and robust .NET library for parsing and validating M3U8 playlists, focusing on HLS and IPTV.
<br />
<a href="https://github.com/bjsneto/M3U8.NET"><strong>Explore the documentation »</strong></a>
<br />
<br />
<a href="https://www.nuget.org/packages/M3U8.NET">View on NuGet</a>
·
<a href="https://github.com/bjsneto/M3U8.NET/issues">Report a Bug</a>
·
<a href="https://github.com/bjsneto/M3U8.NET/issues">Request a Feature</a>
</p>
</div>

---

## Table of Contents

* [About the Project](#about-the-project)
* [Built With](#built-with)
* [Getting Started](#getting-started)
* [Prerequisites](#prerequisites)
* [Installation](#installation)
* [Usage](#usage)
* [Roadmap](#roadmap)
* [Contributing](#contributing)
* [License](#license)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)

---

## About the Project

**M3U8.NET** is a .NET library designed for **efficient parsing and validation** of M3U8 playlist files.  

### Built With

This library is built using the following technologies:

* [.NET 10](https://dotnet.microsoft.com/)
* [xUnit](https://xunit.net/) - Testing framework  
* [FluentAssertions](https://fluentassertions.com/) - For fluent assertions in tests  
* [Bogus](https://github.com/bchavez/Bogus) - For fake data generation in tests  

---

## Getting Started

Follow these instructions to add the library to your project or set up a local development environment.

### Prerequisites

* [.NET SDK 10](https://dotnet.microsoft.com/pt-br/download/dotnet/10.0) 

### Installation

#### Via NuGet

The easiest way to use the library is to install it via the NuGet Package Manager:

Run: `dotnet add package M3U8.NET`

#### Local Development

1. Clone the repository:
```bash
git clone [https://github.com/bjsneto/M3U8.NET.git](https://github.com/bjsneto/M3U8.NET.git)
```
2. Navigate to the project folder and restore dependencies:
```bash
dotnet restore
```
3. Build the project:
```bash
dotnet build
```
4. Run the tests:
```bash
dotnet test
```

---

## Usage

Here’s a basic example of how to parse and validate an M3U8 string

Basic example:

```csharp

// Example M3U8 playlist content
var content = "#EXTM3U\n#EXTINF:-1,Example Channel\nhttp://example.com/stream.ts";

// 1. Load and parse the playlist (parsing and validation happen internally)
Playlist playlist;
try
{
    playlist = Playlist.LoadFromString(content);
    Console.WriteLine("Valid playlist.");
}
catch (Exception ex)
{
    Console.WriteLine($"Error loading playlist: {ex.Message}");
    return;  // Or handle accordingly
}

// 2. Access data
Console.WriteLine($"Number of Segments: {playlist.Segments.Count}");
Console.WriteLine($"First segment title: {playlist.Segments[0].Title}"); // Output: "Example Channel"
```

## Roadmap

The project is constantly evolving. Here are some planned items for future releases:

* [ ] Support for **Master Playlists** (e.g., `#EXT-X-STREAM-INF`, `#EXT-X-MEDIA`, `#EXT-X-I-FRAME-STREAM-INF` for variants, renditions, and groups)
* [ ] Implementation of advanced HLS tags (e.g., `#EXT-X-KEY` for encryption handling with AES-128/SAMPLE-AES, IV, and URI)
* [ ] **Performance optimizations** for parsing large playlists, including streaming-style/incremental parsing
* [ ] Integration and examples with IPTV/HLS monitoring tools
* [ ] M3U8 generation from models (Reverse Parser/Serializer) to serialize playlists back to M3U8 strings
* [ ] Support for **byte ranges** (`#EXT-X-BYTERANGE`) with length and offset handling
* [ ] Handling of **discontinuities** (`#EXT-X-DISCONTINUITY`, `#EXT-X-DISCONTINUITY-SEQUENCE`) for format changes and ad insertions
* [ ] Support for **I-frames and initialization segments** (`#EXT-X-MAP`, `#EXT-X-I-FRAMES-ONLY`)
* [ ] Additional Media Playlist tags (e.g., `#EXT-X-VERSION`, `#EXT-X-TARGETDURATION`, `#EXT-X-MEDIA-SEQUENCE`, `#EXT-X-PROGRAM-DATE-TIME`, `#EXT-X-ENDLIST`, `#EXT-X-PLAYLIST-TYPE`, `#EXT-X-INDEPENDENT-SEGMENTS`)
* [ ] Experimental tags for **ads/interstitials** (e.g., `#EXT-X-CUE-OUT`, `#EXT-X-CUE-IN`)
* [ ] Extensibility for **custom tags** with regex-based parsers, tag mappers, or generic handlers
* [ ] Support for **variable substitution** (`#EXT-X-DEFINE` with QUERYSTRING/IMPORT)
* [ ] **Advanced validations** for semantic consistency (e.g., target duration matching, version compatibility, sequence numbers per RFC 8216)
* [ ] Fluent APIs for **dynamic editing/creation** of playlists (e.g., adding/removing streams, medias, groups)
* [ ] Additional attributes for **media characteristics** (e.g., HDCP-LEVEL, VIDEO-RANGE, FRAME-RATE, CHARACTERISTICS, INSTREAM-ID)

See the **[open issues](https://github.com/bjsneto/M3U8.NET/issues)** for a list of proposed features (and known issues).  
Feel free to open an `Issue` if you have a suggestion!

---

## Contributing

Contributions are what make the open-source community such an amazing place to learn, inspire, and create.  
Your contributions are **greatly appreciated**.

1. **Fork** the Project  
2. Create your **Feature Branch** (`git checkout -b feature/NewFeature`)  
3. Commit your changes (`git commit -m 'feat: Add NewFeature and tests'`)  
4. **Push** to the Branch (`git push origin feature/NewFeature`)  
5. Open a **Pull Request**  

---

## License

Distributed under the MIT License. See `LICENSE` for more information.

---

## Contact
**LinkedIn:** [bjsneto](https://www.linkedin.com/in/bjsneto/)

**Project Link:** [https://github.com/bjsneto/M3U8.NET](https://github.com/bjsneto/M3U8.NET)
