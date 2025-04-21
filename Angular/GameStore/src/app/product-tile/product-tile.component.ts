import { Component, Input } from '@angular/core';
import { Product } from '../interfaces/product';
import { RouterLink } from '@angular/router';
import { PriceComponent } from "../price/price.component";

@Component({
  selector: 'app-product-tile',
  imports: [RouterLink, PriceComponent],
  templateUrl: './product-tile.component.html',
  styleUrl: './product-tile.component.css'
})
export class ProductTileComponent {
  @Input() product: Product = {} as Product;
}
