import { Component, Input } from '@angular/core';
import { Product } from '../interfaces/product';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { timer } from 'rxjs';

@Component({
  selector: 'app-product-tile',
  imports: [RouterLink, CommonModule],
  templateUrl: './product-tile.component.html',
  styleUrl: './product-tile.component.css'
})
export class ProductTileComponent {
  @Input() product: Product = {} as Product;
}
