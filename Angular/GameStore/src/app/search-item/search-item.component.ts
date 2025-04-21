import { Component, Input } from '@angular/core';
import { Product } from '../interfaces/product';
import { RouterLink } from '@angular/router';
import { PriceComponent } from "../price/price.component";

@Component({
  selector: 'app-search-item',
  imports: [RouterLink, PriceComponent],
  templateUrl: './search-item.component.html',
  styleUrl: './search-item.component.css'
})
export class SearchItemComponent {

  @Input() product: Product | null = null;


}
