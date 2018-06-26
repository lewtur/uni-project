export class FullArtistStats {
    artistId: number;
    artistName: string;
    spotifyArtistStats: SpotifyArtistStats[];
    albums: SlimAlbumHeader[];
    artistGigs: EventSummary[];
    tweetSummary: TwitterDaySummary[];
    genre: Genre;
}

export class SlimAlbumHeader {
    name: string;
    releaseDate: Date;
}

export class Genre {
    spotifyGivenGenre: string;
    otherGenresGivenInRelatedArtists: string;
    mostPopularGenreOfRelatedArtists: string;
}

export class AlbumStats {
    header: SpotifyAlbumHeader;
    stats: SpotifyAlbumStats[];
}

export class TwitterDaySummary {
    date: Date;
    tweetCount: number;
    percentage: number;
}

export class SpotifyArtistStats {
    id: number;
    spotifyArtistHeaderId: number;
    followers: number;
    popularity: number;
    datePosted: Date;
}

export class SpotifyAlbumHeader {
    id: number;
    spotifyArtistHeaderId: number;
    spotifyRecordId: string;
    name: string;
    label: string;
    albumType: string;
    releaseDate: string;
    albumArtworkUrl: string;
}

export class SpotifyAlbumStats {
    id: number;
    spotifyAlbumHeaderId: number;
    popularity: number;
    datePosted: Date;
}

export class EventSummary {
    venueName: string;
    venueLocation: string;
    eventName: string;
    startDate: Date;
}

