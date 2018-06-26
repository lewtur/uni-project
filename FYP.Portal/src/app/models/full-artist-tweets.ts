import { AgWordCloudData } from 'angular4-word-cloud';

export class FullArtistTweets {
    tweets: TextAndLink[];
    wordPairings: AgWordCloudData[];
}

export class TextAndLink {
    text: string;
    link: string;
}
