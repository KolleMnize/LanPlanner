import { Routes } from '@angular/router';
import { Home } from './presentation/views/home/home';
import { Events } from './presentation/views/events/events';
import { Catalog } from './presentation/views/catalog/catalog';

export const routes: Routes = [
  { path: 'home', component: Home },
  { path: 'events', component: Events },
  { path: 'catalog', component: Catalog },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: '**',
    redirectTo: 'home',
    pathMatch: 'full',
  },
];
