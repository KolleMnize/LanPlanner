import { Component, signal } from '@angular/core';
import { RouterModule, RouterOutlet } from '@angular/router';
import { UserService } from '../data/user.service';

@Component({
  selector: 'app-root',
  imports: [RouterModule, RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('LanPlanner');
  protected readonly email = signal<string | null>(null);

  constructor(private userService: UserService) {
    this.loadUser();
  }

  private async loadUser(): Promise<void> {
    try {
      const user = await this.userService.getCurrentUser();
      this.email.set(user.email);
    } catch {
      this.email.set(null);
    }
  }
}
