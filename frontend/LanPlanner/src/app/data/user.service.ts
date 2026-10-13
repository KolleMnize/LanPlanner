import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { User, UserResponse } from '../models/user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private api = '/api/users';

  constructor(private http: HttpClient) {}

  async getCurrentUser(): Promise<User> {
    const result = await firstValueFrom(this.http.get<UserResponse>(`${this.api}/me`));

    if (!result?.user) {
      throw new Error('No user returned');
    }

    return result.user;
  }
}
