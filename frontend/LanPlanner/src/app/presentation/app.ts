import { Component, signal } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { UserService } from '../data/user.service';
import { User } from '../models/user.model';
import { HttpErrorResponse } from '@angular/common/http';

@Component({
  selector: 'app-root',
  imports: [RouterModule, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('LanPlanner');
  protected readonly user = signal<User | null>(null);
  protected readonly needsRegistration = signal(false);
  protected readonly registerError = signal<string | null>(null);
  //protected readonly email = signal<string | null>(null);

  constructor(private userService: UserService) {
    this.loadUser();
  }

  private async loadUser(): Promise<void> {
    try {
      const user = await this.userService.getCurrentUser();
      this.user.set(user);
      this.needsRegistration.set(user === null);
    } catch {
      this.user.set(null);
    }
  }
  protected async register(name: string): Promise<void> {
    this.registerError.set(null);
    try {
      this.user.set(await this.userService.register(name));
      this.needsRegistration.set(false);
    } catch (e) {
      const detail = e instanceof HttpErrorResponse ? e.error?.detail : null;
      this.registerError.set(detail ?? 'Registrierung fehlgeschlagen.');
    }
  }
}
