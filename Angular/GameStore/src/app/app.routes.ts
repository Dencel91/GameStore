import { Routes } from '@angular/router';
import { ProductListComponent } from './product-list/product-list.component';
import { ProductPageComponent } from './product-page/product-page.component';
import { CartPageComponent } from './cart-page/cart-page.component';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { SignUpComponent } from './sign-up/sign-up.component';
import { LibraryPageComponent } from './library-page/library-page.component';
import { ProfilePageComponent } from './profile-page/profile-page.component';
import { AuthGuard } from './guards/auth.guard';
import { AdminGuard } from './guards/admin.guard';
import { AddProductPageComponent } from './add-product-page/add-product-page.component';
import { EditProductPageComponent } from './edit-product-page/edit-product-page.component';
import { SearchPageComponent } from './search-page/search-page.component';
import { LoginPageComponent } from './login-page/login-page.component';

export const routes: Routes = [
    {path: '', redirectTo: 'store', pathMatch: 'full'},
    {path: 'store', component: ProductListComponent},
    {path: 'product/:id', component: ProductPageComponent},
    {path: 'search/:term', component: SearchPageComponent},
    {path: 'cart', component: CartPageComponent},
    {path: 'login', component: LoginPageComponent, canActivate: [AuthGuard]},
    {path: 'signup', component: SignUpComponent, canActivate: [AuthGuard]},
    {path: 'library', component: LibraryPageComponent, canActivate: [AuthGuard]},
    {path: 'profile', component: ProfilePageComponent, canActivate: [AuthGuard]},
    {path: 'add-product', component: AddProductPageComponent, canActivate: [AdminGuard]},
    {path: 'edit-product/:id', component: EditProductPageComponent, canActivate: [AdminGuard]},
    {path: '**', component: PageNotFoundComponent},
];
