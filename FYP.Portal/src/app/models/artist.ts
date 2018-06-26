export class Artist {
    id: number;
    name: string;
    spotifyRecordId: string;

    constructor()
    constructor(name: string = null) {
       if (name) {
           this.name = name;
       }
    }
}
