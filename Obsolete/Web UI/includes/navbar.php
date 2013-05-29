<div class="navbar navbar-fixed-top">
  <div class="navbar-inner">
    <div class="container-fluid">
      <a class="btn btn-navbar" data-toggle="collapse" data-target=".nav-collapse">
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
        <span class="icon-bar"></span>
      </a>
      <a class="brand" href="home.php">Open Source Automation</a>
      <div class="nav-collapse">
        <ul class="nav">
          <li <?php if (curPageName() == 'home.php') { ?>class="active"<?php } ?>><a href="home.php">Home</a></li>
          <li <?php if (curPageName() == 'screens.php') { ?>class="active"<?php } ?>><a href="screens.php">Screens</a></li>
          <li <?php if (curPageName() == 'plugins.php') { ?>class="active"<?php } ?>><a href="plugins.php">Plugins</a></li>
          <li <?php if (curPageName() == 'analytics.php') { ?>class="active"<?php } ?>><a href="analytics.php">Analytics</a></li>
        </ul>
      </div><!--/.nav-collapse -->
    </div>
  </div>
</div>