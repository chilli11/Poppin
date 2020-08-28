import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';

export default class AdminRoute extends Route {
	@service accountService;
	beforeModel() {
		this.accountService.isAuthenticated()
			.catch(() => this.transitionTo('account.login'));
	}
}
