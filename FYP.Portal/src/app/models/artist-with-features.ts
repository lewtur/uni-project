import { Artist } from './artist';
import { Genre } from './full-artist-stats';

export class UserRecommendedArtist {
    artist: Artist;
    features: SinglePopularityFeature[];
    matchedGenres: string[];
    genre: Genre;

    genreList = [];
    selected = false;
}

export class SinglePopularityFeature {
    score: number;
    term: string;
    averageMagnitude: number;
    rank: number;

    firstHalf = '';
    secondHalf = '';
}
