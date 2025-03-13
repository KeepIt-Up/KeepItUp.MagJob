import { Injectable } from '@angular/core';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';

@Injectable({
    providedIn: 'root'
})
export class ImageService {
    constructor(private sanitizer: DomSanitizer) { }

    getSafeImageUrl(base64String: string | undefined): SafeUrl | undefined {
        if (!base64String) return undefined;

        try {
            const imageUrl = `data:image/jpeg;base64,${base64String}`;
            return this.sanitizer.bypassSecurityTrustUrl(imageUrl);
        } catch (error) {
            console.error('Error processing image:', error);
            return undefined;
        }
    }
} 