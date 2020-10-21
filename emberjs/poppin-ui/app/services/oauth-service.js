import Service, { inject as injectService } from '@ember/service';
import HttpResources from '../utils/http-resources';
import { tracked } from '@glimmer/tracking';
import { Promise } from 'rsvp';

const checkFBLoginState = scope => () => {
	return new Promise((res) => {
		return window.FB ? window.FB.getLoginStatus(function(response) {
			scope.fbAuth = response;
			return res(response);
		}) : res(false);
	});
}

export default class OauthServiceService extends Service {
	@injectService apiService;
	@injectService accountService;
	@injectService router;

	@tracked fbAuth;
	checkFBLoginState = checkFBLoginState(this);
	
	isOAuthenticated() {
		return this.checkFBLoginState().then((data) => {
			if (data.status == 'connected') {
				return this.authorizeFacebook(data.authResponse.accessToken);
			}
			return Promise.reject();
		});
	}
	
	authorizeFacebook(accessToken) {
		return this.apiService.request({
			resource: HttpResources.authorizeFacebook,
			body: { accessToken }
		}).then((response) => {
			sessionStorage.setItem('poppin_jwt', response.token);
			this.apiService.jwt = response.token;
			this.accountService.authInfo = { authorized: true }

			this.router.transitionTo('account.me');
			return response.token;
		});
	}
}
