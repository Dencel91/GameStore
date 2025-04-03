import { Component, Input } from '@angular/core';
import { Product } from '../interfaces/product';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-search-item',
  imports: [RouterLink],
  templateUrl: './search-item.component.html',
  styleUrl: './search-item.component.css'
})
export class SearchItemComponent {

  @Input() product: Product | null = null;


}
