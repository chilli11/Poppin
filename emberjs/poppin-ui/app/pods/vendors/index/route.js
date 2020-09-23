import Route from '@ember/routing/route';
import { inject as service } from '@ember/service';
import { computed } from '@ember/object';

export default class VendorsIndexRoute extends Route {
	@service vendorsService;
	@service accountService;
	
	@computed('accountService.accountInfo')
	get accountInfo() {
		return this.accountService.accountInfo;
	}
	@computed('accountService.profile')
	get profile() {
		return this.accountService.profile;
	}
	get isAdmin() {
		const accAdmin = this.accountInfo && this.accountInfo.role == 'Admin';
		const profAdmin = this.profile && this.profile.role == 'Admin';
		return accAdmin || profAdmin;
	}

	getVendors() {
		return this.isAdmin ? this.vendorsService.getAllVendors() : this.vendorsService.getMyVendors();
	}

	model() {
		return this.getVendors()
	}
}
