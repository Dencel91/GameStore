import { ProductReview } from "./productReview";

export interface Product {
    id: number;
    name: string;
    description: string;
    price: number;
    thumbnailUrl: string;
    images: string[]
    reviews: ProductReview[]
}