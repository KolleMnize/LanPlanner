import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { User, UserResponse } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private api = '/api/users';

  constructor(private http: HttpClient) {}

  async getCurrentUser(): Promise<User | null> {
    try {
      const result = await firstValueFrom(this.http.get<UserResponse>(`${this.api}/me`));

      return result.user;
    } catch (e) {
      if (e instanceof HttpErrorResponse && e.status === 404) {
        return null;
      }
      throw e;
    }
  }

  async register(name: string): Promise<User> {
    const result = await firstValueFrom(this.http.post<UserResponse>(`${this.api}/me`, { name }));
    return result.user;
  }
}
