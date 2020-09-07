import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AccountIndexRoute extends Route {
	@service accountService;
	model() {
		const isAuthorized = this.accountService.authInfo && this.accountService.authInfo.authorized;
		return isAuthorized ? this.accountService.me() : null;
	}
}
