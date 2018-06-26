import { Artist } from './artist';
import { DetailedArtist } from './detailed-artist';

export class DisplayGig {
    eventName: string;
    venueName: string;
    startTime: string;
    eventDate: Date;
    description: string;
    venueLatitude: number;
    venueLongitude: number;
    town: string;
    artistSpotifyUrl: string;
    artistGenres: string[];
    artistGoogleLink: string;
}

export class Gig {
    event: Event;
    venue: Venue;
    artist: DetailedArtist[];
}

export class Event {
    name: string;
    cancelled: boolean;
    venueId: number;
    startDate: Date;
    endDate: Date;
    description: string;
    doorsOpen: string;
    doorsClose: string;
    lastEntry: string;
    minAge: number;
    entryPrice: string;
}

export class Venue {
    id: number;
    name: string;
    address: string;
    town: string;
    postCode: string;
    latitude: string;
    longitude: string;
}
