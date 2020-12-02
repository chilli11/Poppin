import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AdminRoute extends Route {
	@service accountService;
	beforeModel() {
		return this.accountService.isAuthenticated()
			.then(authInfo => authInfo.authorized ? this.accountService.myProfile() : null);
	}
}
