import { Routes } from '@angular/router';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductPageComponent } from './product-page/product-page.component';
import { CartPageComponent } from './cart-page/cart-page.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { SignUpComponent } from './sign-up/sign-up.component';

export const routes: Routes = [
    {path: '', redirectTo: 'store', pathMatch: 'full'},
    // {path: 'products', loadChildren: './product-list/product-list.module#ProductListModule'},
    {path: 'store', component: ProductListComponent},
    {path: 'product/:id', component: ProductPageComponent},
    {path: 'cart', component: CartPageComponent},
    {path: 'signUp', component: SignUpComponent},
    {path: '**', component: PageNotFoundComponent},
];
