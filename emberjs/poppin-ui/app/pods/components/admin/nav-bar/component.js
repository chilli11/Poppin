import Component from '@glimmer/component'
import { inject as service } from '@ember/service';
import { computed, action } from '@ember/object';

export default class NavBarComponent extends Component {
	@service accountService;
	@service router;

	@computed('accountService.authInfo')
	get authInfo() {
		return this.accountService.authInfo;
	}
	@computed('accountService.accountInfo')
	get accountInfo() {
		return this.accountService.accountInfo;
	}
	@computed('accountService.profile')
	get profile() {
		return this.accountService.profile;
	}

	get authorized() {
		return this.authInfo && this.authInfo.authorized;
	}
	get isAdmin() {
		const accAdmin = this.accountInfo && this.accountInfo.role == 'Admin';
		const profAdmin = this.profile && this.profile.role == 'Admin';
		return accAdmin || profAdmin;
	}

	@action
	logout() {
		return this.accountService.logout()
			.then(this.router.transitionTo('account'));
	}
}