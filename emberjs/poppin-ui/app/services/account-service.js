import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';
import { tracked } from '@glimmer/tracking';
import { Promise } from 'rsvp';

export default class AccountService extends Service {
	@injectService apiService;
	@injectService session;

	@tracked authInfo = null

	isAuthenticated() {
		if (this.authInfo) {
			if (this.authInfo.authorized) return Promise.resolve();
			return Promise.reject();
		}
		return this.apiService.request({
			resource: HttpResources.isAuthenticated
		}).then(() => this.authInfo = { authorized: true })
			.catch(() => this.authInfo = { authorized: false });
	}

	registerAccount(credentials) {
		return this.apiService.request({
			resource: HttpResources.registerAccount,
			body: credentials
		});
	}

	login(credentials) {
		return this.apiService.request({
			resource: HttpResources.login,
			body: credentials
		}).then((response) => {
			sessionStorage.setItem('poppin_jwt', response.token);
			this.apiService.jwt = response.token;
			return response.token;
		});
	}

	me() {
		return this.apiService.request({
			resource: HttpResources.myAccount,
		}).then(({ user }) => user);
	}

	logout() {
		this.apiService.jwt = null;
		sessionStorage.removeItem('poppin_jwt');
		return Promise.resolve();
	}
}
